# Lykke.Ico.KeyGenerator
CLI tool to generate BTC and ETH key pairs and addresses. Generates a couple of CSV files - one with public keys only, and one with full data including private (secret) keys. Optionally BTC and ETH addresses can be included. Symbol `;` is used as separator to make generated files compatible with spreadsheet tools.

## Building
Utility is written with .NET Core and can be built for multiple platforms:

    src\Lykke.Ico.KeyGenerator> dotnet publish -c Release -r win10-x64
    src\Lykke.Ico.KeyGenerator> dotnet publish -c Release -r osx.10.12-x64

Those commands generate output for corresponding platforms which contain all required binaries:

    src\Lykke.Ico.KeyGenerator\bin\Release\netcoreapp2.0\win10-x64\
    src\Lykke.Ico.KeyGenerator\bin\Release\netcoreapp2.0\osx.10.12-x64\

## Usage
    Lykke.Ico.KeyGenerator -n <Number of key pairs to generate> [-p <Path to public keys file>] [-s <Path to secret keys file>] [-net <Name of BTC net>] [-a] [-h]

## Arguments

#### `-n <Number of key pairs to generate>` 
Mandatory. Number of BTC and ETH key pairs to generate. Positive integer.

#### `-p <Path to public keys file>`
Optional. Path to file to write public keys. If not specified then "public.csv" in application working directory is used.

#### `-s <Path to secret keys file>`
Optional. Path to file to write private (secret) keys. If not specified then "secret.csv" in application working directory is used.

#### `-net <Name of BTC network>`
Optional. Name of BTC network to generate BTC keys for. Available values - `Main`, `TestNet`, `RegTest`. 
If not specified or corresponding network is not found by name then `Main` network is used.

#### `-a`
Optional. If specified then addresses are generated in addition to public and private keys.

#### `-h`
Optional. If specified then headers are writed into generated CSV files. By default headers are omitted.

#### `-e <Entropy>`
Optional. Additional entropy for RNG.

Default columns order is: 
- For public keys file: [btcAddress]; btcPublic; [ethAddress]; ethPublic;
- For secret keys file: [btcAddress]; btcPublic; btcPrivate; [ethAddress]; ethPublic; ethPrivate;

Addresses are optional and are writed only if `-a` argument is specified.

