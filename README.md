# ALSI.PrimitiveValidation

[![NuGet Version](https://img.shields.io/nuget/v/ALSI.PrimitiveValidation.svg?style=flat)](https://www.nuget.org/packages/ALSI.PrimitiveValidation/)
[![Build Status](https://github.com/alsi-lawr/alsi.primitivevalidation/actions/workflows/deploy-nuget.yml/badge.svg)](https://github.com/alsi-lawr/ALSI.PrimitiveValidation/actions)
[![Downloads](https://img.shields.io/nuget/dt/ALSI.PrimitiveValidation.svg?logo=nuget&logoSize=auto)](https://www.nuget.org/packages/ALSI.PrimitiveValidation)
[![codecov](https://codecov.io/gh/alsi-lawr/alsi.primitivevalidation/graph/badge.svg)](https://codecov.io/gh/alsi-lawr/alsi.primitivevalidation)

**ALSI.PrimitiveValidation** is a lightweight C# library for primitive data validation.

## Installation

Add **ALSI.PrimitiveValidation** to your project by adding it as a NuGet package:

```sh
dotnet add package ALSI.PrimitiveValidation
```

## Features

### Data Validation Attributes

This library provides a set of attributes to validate different primitive conditions:

- **`IsBase64Attribute`**: Ensures that a property, field, or parameter is a valid Base64-encoded string.
- **`PropertyNotNullAttribute`**: Ensures that a property, field, or parameter is initialized with a non-null value.
- **`StringNotNullOrEmptyAttribute`**: Ensures that a property, field, or parameter is not null and not an empty string.

These attributes can be used to decorate model properties or record parameters, enforcing validation automatically via the .NET data annotations framework.

## Usage Example

### Validation Attributes

Using this model:

```csharp
using ALSI.PrimitiveValidation;

public record MyModel(
    [property: IsBase64] string EncodedData,
    [property: PropertyNotNull] object ImportantProperty,
    [property: StringNotNullOrempty] string Name);
```

We can validate manually, like so:

```csharp
var model = new MyModel("SGVsbG8sIHdvcmxkIQ==", new (), "Alice");

Validator.ValidateObject(
    model,
    new ValidationContext(model),
    validateAllProperties: true
);
```
