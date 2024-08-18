[<AutoOpen>]
module Cloud.Ops

open Pulumi

let getConfigOrDefault (config: Config) =
    (fun key defaultValue ->
        match config.Get key with
        | null -> defaultValue
        | value -> value)
