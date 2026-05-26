namespace AICompanyOS.Runtime.Tools;

/// <summary>
/// Registry of all tools available to the runtime execution engine.
///
/// The registry is populated at startup by the DI container.
/// Tools are registered by name and resolved by the <see cref="IToolExecutor"/>
/// when processing tool call requests from the LLM.
///
/// The registry is read-only at runtime — tools cannot be added or removed
/// after the application starts.
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// Retrieves a tool by its registered name.
    /// Returns null if no tool with that name is registered.
    /// </summary>
    ITool? Resolve(string toolName);

    /// <summary>
    /// Returns all registered tools.
    /// Used to build the tool list included in LLM system prompts.
    /// </summary>
    IReadOnlyList<ITool> GetAll();

    /// <summary>
    /// Returns all tools available to a specific agent role.
    /// Allows role-based tool access control (e.g., only QA agents can file bugs).
    /// </summary>
    IReadOnlyList<ITool> GetForRole(string agentRole);

    /// <summary>Returns true if a tool with the given name is registered.</summary>
    bool IsRegistered(string toolName);
}
