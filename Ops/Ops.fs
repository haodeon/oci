[<AutoOpen>]
module Cloud.Ops

open Pulumi

let getConfigOrDefault (config: Config) =
    fun key defaultValue ->
        match config.Get key with
        | null -> defaultValue
        | value -> value

let getOutputDetails (stackRef: StackReference) =
    fun convert outputKey ->
        task {
            // Use GetOutputDetailsAsync and return the result
            let! stackOutput = stackRef.GetOutputDetailsAsync(outputKey)
            return convert stackOutput.Value
        }

let objToString (obj: obj) = obj :?> string
let objToFloat (obj: obj) = obj :?> float
