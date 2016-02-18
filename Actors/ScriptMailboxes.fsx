#r @"bin/debug/Actors.exe"
open Mailboxes

let console =
    let agent =
        Agent("console-writer", (fun (agent:AgentRef<string>) ->
            let rec loop() =
                async {
                    let! msg = agent.Receive()
                    printfn "%s" msg
                    return! loop()
                }
            loop()
        ))
    agent.Start()
    agent

console.Post("Writing through an agent")

