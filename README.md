# Avra Code gen
## Install
```powershell
dotnet tool install Chr.Avro.Cli --global
```
## Example calls for the workshop
```powershell
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100073 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100072 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100074 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
```

## Write Classes
```powershell
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100072 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\PresentRecipient.cs
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100073 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\OrderedPresent.cs
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ --id 100074 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\OrderedPresentChecked.cs
```