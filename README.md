[<img src="https://finnpersonalserver.visualstudio.com/_apis/public/build/definitions/9c406e00-f146-446c-aa58-e3030f5de520/2/badge"/>](https://finnpersonalserver.visualstudio.com/_apis/_build/index?definitionId=9c406e00-f146-446c-aa58-e3030f5de520)

# IOTA C# Library

This is an unofficial C#-Library to interact with the Iota network. It targets .NET Standard 2.0 which means it can be used within .NET Framework-, .NET Core- and Xamarin-Projects. It supports all 14 [Core API calls](https://iota.readme.io/docs/getnodeinfo) aswell as the [Proposed calls](https://github.com/iotaledger/wiki/blob/master/api-proposal.md). All API calls are also available as asynchronous methods.
This library is actively supported (as of 2017-02-10). I will try to implement all upcoming Iota-Features as soon as possible. Please note that the Iota project is still in Beta and so is this API. (I am not responsible for any kind of loss of your Iota ;))

## Getting Started

This is how you create the API you can use within your projects:

```csharp
   IotaApi iotaApi = new IotaApi("nodes.thetangle.org", 443, true);
   //...
```
## Join the Iota Community

- [Iota.org](https://iota.org) 
- [Twitter](https://twitter.com/iotatoken)
- [Discord](https://discordapp.com/channels/397872799483428865/398452378333872138)
- [Youtube Tutorials](https://www.youtube.com/watch?v=MsaPA3U4ung&list=PLmL13yqb6OxdIf6CQMHf7hUcDZBbxHyza&index=1)

## Additional Notes and Credits

I created this API to reach more developers creating great stuff based on Iota. Since C# is a very clean and expressive language this API is also a great way to learn how Iota works under the hood.

Special Thanks to:
- [MobileFish](https://www.youtube.com/channel/UCG5_CT_KjexxjbgNE4lVGkg): His YT-Channel was my #1 source when I was new to Iota.
- [Official Library](https://github.com/iotaledger/iota.lib.csharp): The official library served as skeleton for this API.
- [Felandil](https://github.com/Felandil): He created a C# library based on .NET Framework where I was able to adapt some functions.

## Donate: 
ESEQEIEMRHISAUWIZUPIVPSIGDPVSGNBDBSZWUMCUPYDFHYFQAKBXJVQWRUXBKBJDGUWOCDW9SVQDVFH9PAFZJDVYZ
