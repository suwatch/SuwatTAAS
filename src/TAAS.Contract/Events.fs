module TAAS.Contract.Events

open Types

type Event =
    | AccountCreated of AccountId * AccountName
    | UserAddedToAccount of UserId * UserName * PasswordHash * AccountId
