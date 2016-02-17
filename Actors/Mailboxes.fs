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
