module Mailboxes

type IAsyncReplyChannel<'a> = 
    abstract Reply : 'a -> unit

[<AbstractClass>]
type AgentRef(id:string) =
    member val Id = id with get, set
    abstract Start : unit -> unit

[<AbstractClass>]
type AgentRef<'a>(id:string) =
    inherit AgentRef(id)
    abstract Receive : unit -> Async<'a>
    abstract Post : 'a -> unit
    abstract PostAndTryAsyncReply : (IAsyncReplyChannel<'b> -> 'a) -> Async<'b option>

type MailboxReplyChannel<'a>(asyncReplyChannel:AsyncReplyChannel<'a>) =
    interface IAsyncReplyChannel<'a> with
        member x.Reply(msg) = asyncReplyChannel.Reply(msg)

type Agent<'a>(id:string, comp, ?token) =
    inherit AgentRef<'a>(id)
    let mutable agent:MailboxProcessor<'a> option = None
    override x.Receive() = agent.Value.Receive()
    override x.Post(msg:'a) = agent.Value.Post(msg)
    override x.PostAndTryAsyncReply(builder) =
        agent.Value.PostAndTryAsyncReply(fun rc ->
            builder(new MailboxReplyChannel<_>(rc)))
    override x.Start() =
        let mbox = MailboxProcessor.Start((fun inbox ->
            comp (x :> AgentRef<_>)), ?cancellationToken = token)
        agent <- Some mbox
