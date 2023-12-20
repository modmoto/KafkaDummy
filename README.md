# Avra Code gen (does not work with the offical afro deserializer)
## Install
```powershell
dotnet tool install Chr.Avro.Cli --global
```
## Example calls for the workshop
```powershell
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100073 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100072 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100074 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud
```

## Write Classes
```powershell
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100072 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\PresentRecipient.cs
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100073 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\OrderedPresent.cs
dotnet avro generate --registry-config schema.registry.basic.auth.user.info=VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K --id 100074 --registry-url https://psrc-2312y.europe-west3.gcp.confluent.cloud | Out-File .\Shared\OrderedPresentChecked.cs
```

# Avra Codegen Official with file
more here
https://github.com/confluentinc/confluent-kafka-dotnet/tree/master/examples/AvroSpecific
## Install
```powershell
dotnet tool install --global Apache.Avro.Tools --version 1.11.3
```

##
```powershell
avrogen -s .\Shared\AvroFile\schema-factory.presents.checked.0-value-v1.avsc .\Shared\Generated\
avrogen -s .\Shared\AvroFile\schema-factory.presents.ordered.0-value-v1.avsc .\Shared\Generated\
avrogen -s .\Shared\AvroFile\schema-factory.presents.recipients.0-value-v2.avsc .\Shared\Generated\
```
