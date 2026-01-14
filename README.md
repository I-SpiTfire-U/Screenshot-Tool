# Screenshot-Tool

> A super basic screen-shotting utility for Linux that uses Grim and Slurp. 

## Building and Running

**1.** Clone the Repository

```sh
git clone https://github.com/I-SpiTfire-U/Screenshot-Tool
cd Screenshot-Tool
```

**2.** Build and Run

```sh
dotnet build
dotnet run
```

**3.** Publish a Standalone Binary

Select the runtime ID for whichever platform you are building for, a list of available runtime ID's can be found here: [RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

```sh
dotnet publish -c Release -r <RID> --self-contained -o out
```

