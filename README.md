[![](https://img.shields.io/nuget/v/soenneker.maf.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.openai/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.openai/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.maf.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.openai/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.openai/actions/workflows/codeql.yml)

# Soenneker.Maf.Pool.OpenAI

Provides OpenAI-specific registration extensions for `IMafPool`, enabling integration via Microsoft Agent Framework.

## Install

```bash
dotnet add package Soenneker.Maf.Pool.OpenAI
```

## Usage

```csharp
using Soenneker.Maf.Pool.OpenAI;
using Soenneker.Maf.Pool.Abstract;

await pool.AddOpenAI(
    poolId: "chat",
    key: "openai-primary",
    modelId: "gpt-5-mini",
    apiKey: configuration["OPENAI_API_KEY"]!,
    rpm: 60,
    instructions: "Answer concisely.",
    cancellationToken: cancellationToken);

(AIAgent? agent, IMafPoolEntry? entry) =
    await pool.GetAvailable("chat", cancellationToken);
```

Omit `endpoint` for OpenAI's default service. A custom endpoint must expose an API compatible with the OpenAI .NET chat client.

## What you get

- `MafPoolOpenAIExtension` — Provides OpenAI-specific registration extensions for `IMafPool`, enabling integration via Microsoft Agent Framework.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `MafPoolOpenAIExtension.AddOpenAI(pool, poolId, key, modelId, apiKey, endpoint, rps, rpm, rpd, tokensPerDay, instructions, cancellationToken)` | Registers an OpenAI model in the agent pool with optional rate/token limits. | A task that completes when the openai addition is complete. |
| `MafPoolOpenAIExtension.RemoveOpenAI(pool, poolId, key, cancellationToken)` | Unregisters an OpenAI model from the agent pool and removes the associated cache entry. | True if the entry existed and was removed; false if it was not present. |

## Practical notes

- The agent is created lazily and reused until its entry is removed.
- Store the API key in a secret provider; the pool retains it in the entry options while the entry is registered.
- Omitted instructions default to `You are a helpful assistant.`
- Checkout consumes one request from the configured quota. `tokensPerDay` is not reconciled against actual provider token usage.
