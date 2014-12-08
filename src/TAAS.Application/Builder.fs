module TAAS.Application.Builder
open System
open System.Security.Cryptography;

open EventStore.ClientAPI

open TAAS.Contract
open Types
open Commands

open TAAS.Domain
open State
open CommandHandling

open TAAS.Infrastructure
open ApplicationBuilder
open Railroad
open EventStore
open EventStore.EventStore


//http://www.obviex.com/samples/hash.aspx
let generateSalt = 
    let minSaltSize = 4
    let maxSaltSize = 8
    let random = new Random()
    let saltSize = random.Next(minSaltSize, maxSaltSize)
    let saltBytes = Array.init saltSize (fun _ -> byte 0)
    let rng = new RNGCryptoServiceProvider()
    rng.GetNonZeroBytes(saltBytes) |> ignore
    saltBytes

let hasher s = 
    let getbytes (s:string) = (System.Text.Encoding.UTF8.GetBytes(s), generateSalt)
    let concatBytes (frontBytes, backBytes) = (frontBytes, backBytes, Array.concat [frontBytes;backBytes])
    let computeHash hashMethod (bytes, saltBytes, allBytes) = (bytes, saltBytes, hashMethod allBytes)
    let addSaltToHash (bytes, saltBytes, allBytes) = Array.concat [allBytes;saltBytes]

    use algorithm = new System.Security.Cryptography.SHA512Managed()
    let hasher (b:byte array) = algorithm.ComputeHash(b)
    s |> (getbytes >> concatBytes >> (computeHash hasher) >> addSaltToHash >> System.Convert.ToBase64String)

let createApplication = 
    let es = connect()
    let appendStream = appendToStream es
    let readStream = readFromStream es
    let passwordToString (Password s) = s
    let toPasswordHash s = PasswordHash s
    let dependencies = {
        Hasher = passwordToString >> hasher >> toPasswordHash
        ReadEvents = (fun id -> readStream (toStreamId id))
    }

    let handler = handle dependencies
    let save (id, version, events) = appendStream (toStreamId id) version events

    buildApplication save handler

//
//let toStreamId id = sprintf "Data-%O" id
//
//let createApplication readStream appendStream = 
//    let aggId command = 
//        match command with
//        | AccountCommand(CreateAccount(AccountId id, _)) -> id
//        | UserCommand(AddUserToAccount(UserId id, _, _, _)) -> id
//
////    let loadState command id = 
////        let streamId = toStreamId id
////        let (version, events) = readStream streamId
////        let state = evolve command events
////        (version, state)
//        
//    let save id expectedVersion newEvents =
//        let streamId = toStreamId id
//        appendStream streamId expectedVersion newEvents
//
//    let defaultDependencies = {
//        Hasher = (fun (Password x) -> PasswordHash x);
//        ReadEvents = 
//    }
//
//    fun command ->
//        let aggregateId = aggId command
//        let state = loadState command aggregateId 
//        state ||> fun version state -> (version, handle defaultDependencies command state) ||> save aggregateId 
//    fun command -> command