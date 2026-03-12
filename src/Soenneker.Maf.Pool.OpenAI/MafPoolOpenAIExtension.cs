using System;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Soenneker.Maf.Dtos.Options;
using Soenneker.Maf.Pool.Abstract;

namespace Soenneker.Maf.Pool.OpenAI;

/// <summary>
/// Provides OpenAI-specific registration extensions for <see cref="IMafPool"/>, enabling integration via Microsoft Agent Framework.
/// </summary>
public static class MafPoolOpenAIExtension
{
    /// <summary>
    /// Registers an OpenAI model in the agent pool with optional rate/token limits.
    /// </summary>
    public static ValueTask AddOpenAI(this IMafPool pool, string poolId, string key, string modelId, string apiKey, string? endpoint = null,
        int? rps = null, int? rpm = null, int? rpd = null, int? tokensPerDay = null, string? instructions = null,
        CancellationToken cancellationToken = default)
    {
        var options = new MafOptions
        {
            ModelId = modelId,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            AgentFactory = (opts, _) =>
            {
                OpenAIClient client = string.IsNullOrEmpty(opts.Endpoint)
                    ? new OpenAIClient(new ApiKeyCredential(opts.ApiKey!))
                    : new OpenAIClient(new ApiKeyCredential(opts.ApiKey!), new OpenAIClientOptions { Endpoint = new Uri(opts.Endpoint!, UriKind.Absolute) });
                var chatClient = client.GetChatClient(opts.ModelId!);
                IChatClient ichatClient = chatClient.AsIChatClient();
                AIAgent agent = ichatClient.AsAIAgent(instructions: instructions ?? "You are a helpful assistant.", name: opts.ModelId);
                return new ValueTask<AIAgent>(agent);
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an OpenAI model from the agent pool and removes the associated cache entry.
    /// </summary>
    /// <returns>True if the entry existed and was removed; false if it was not present.</returns>
    public static ValueTask<bool> RemoveOpenAI(this IMafPool pool, string poolId, string key, CancellationToken cancellationToken = default)
    {
        return pool.Remove(poolId, key, cancellationToken);
    }
}
