module Cloud.Iam

open Pulumi.FSharp
open Pulumi.Oci.Identity


let createCompartment cmpName cmpId description =
    Compartment(cmpName, CompartmentArgs(CompartmentId = cmpId, Description = description, Name = cmpName))

let infra () =
    let stackConfig = Pulumi.Config()
    let getConfig = getConfigOrDefault stackConfig

    let rootCmpId = stackConfig.Require "rootCmpId"
    let networkCmpDesc = getConfig "networkCmpDesc" "Networking resources"
    let managementCompDesc = getConfig "managementCmpDesc" "Infrastructure services"

    // Add your resources here.
    let networkCompartment = createCompartment "network-cmp" rootCmpId networkCmpDesc

    let managementCompartment =
        createCompartment "mgmt-cmp" rootCmpId managementCompDesc

    // Export outputs here.
    dict
        [ ("networkCmpId", networkCompartment.Id :> obj)
          ("managementCmpId", managementCompartment.Id) ]

[<EntryPoint>]
let main _ = Deployment.run infra
