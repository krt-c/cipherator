# cif

Secret key command line encryption decryption nuget tool.

## Installation

dotnet tool install -g ktc.cif --version 0.0.7-dev

## Update

dotnet tool update -g ktc.cif --version 0.0.7-dev

## Remove

dotnet tool uninstall -g ktc.cif --version 0.0.7-dev

## Commands

### Encrypt Command

- Encrypt a text with the provided secret key. `cif encrypt -secret passkey -text some_text`
- Encrypt a list of strings in a text file with the provided secret key. `cif encrypt -secret passkey -file C:\someFile.txt`

### Decryption Command

- Decrypt a cipher with the provided secret key. `cif decrypt -secret passkey -cipher 61jIjXTn1aVVuydV4fgCs/15V6E3c1UeBxgk4b/WHv9zmBqWwof+u/KHPMnHf1VS`
- Decrypt a list of ciphers in a text file with the provided secret key. `cif decrypt -secret passkey -file C:\someFile.txt`

### Set Secret Command

- The secret can be set globally so it is not required when encrypting and decrypting. Secret set at user level. `cif set -secret someSecret`

### Get Secret Command

- Get the set secret. `cif get`
