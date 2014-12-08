module TAAS.Domain.EventHandling
//open TAAS.Contract
//open Account
//open Commands
//open State
//
//let evolve command events = 
//    let initialState = match command with
//                       | AccountCommand(_) -> State.Account(initAccount)
//                       | UserCommand(_) -> State.User(initUser)
//    let evolveOne state event =
//        match state with
//        | State.Account(p) -> evolveAccount p event
//        | _ -> state
//    List.fold evolveOne initialState events
