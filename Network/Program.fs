module Cloud.Network

open Pulumi
open Pulumi.FSharp
open Pulumi.Oci.Core

let createVcn cmpId vcnName cidrs dnsLabel =
    VirtualNetwork(
        vcnName,
        VirtualNetworkArgs(
            CompartmentId = cmpId,
            CidrBlocks = toInputList cidrs,
            DisplayName = vcnName,
            DnsLabel = dnsLabel,
            IsIpv6enabled = true
        )
    )

let createSubnet cmpId vcnId subnetName cidr ipv6 dnsLabel ingress =
    Subnet(
        subnetName,
        SubnetArgs(
            CompartmentId = cmpId,
            VcnId = vcnId,
            CidrBlock = cidr,
            DisplayName = subnetName,
            DnsLabel = dnsLabel,
            Ipv6cidrBlock = ipv6,
            ProhibitInternetIngress = ingress
        )
    )

let infra () =
    let stackConfig = Config()
    let getConfig = getConfigOrDefault stackConfig

    let iamStackRef =
        let org = Deployment.Instance.OrganizationName
        let stack = Deployment.Instance.StackName

        StackReference($"{org}/oci-iam/{stack}")
        |> fun stackRef -> getOutputDetails stackRef objToString

    let cidrs = stackConfig.RequireObject "cidrs"
    let vcnLabel = stackConfig.Require "vcnLabel"
    let vcnName = getConfig "vcnName" "vcn"
    let networkCmpId = iamStackRef "networkCmpId"

    // Add your resources here.
    let vcn = createVcn networkCmpId.Result vcnName cidrs vcnLabel

    let subnetPublic =
        createSubnet networkCmpId.Result vcn.Id "public-sn" "10.100.1.0/24" "2603:c024:c011:52:::/64" "public1" true

    // Export outputs here.
    dict []

[<EntryPoint>]
let main _ = Deployment.run infra
