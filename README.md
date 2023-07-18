# EULYNX Live Planning Toolkit

This is a helper library for creating EULYNX DataPrep documents from code.

This is currently based on EULYNX DataPrep 1.1.

To be able to use this library, you need to have access to the EulynxDpLibrary NuGet repository:


```
dotnet nuget add source \
    https://gitlab.com/api/v4/projects/30224934/packages/nuget/index.json \
    -n "EULynxDataPrep" --username <username> --password <token> --store-password-in-clear-text
```

## Custom Classes

For certain use cases, we introduce a set of non-standard data classes.

 - GerEtcsBaliseGroup
 - RastaAxleCountingSection
 - RastaSignal
 - RastaTurnout

# Examples

See the [Examples](Examples) folder.
