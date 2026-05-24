namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Defines the role of an AI agent within the company OS.
/// Each role carries specific responsibilities and permissions.
/// </summary>
public enum AgentRole
{
    /// <summary>Chief Executive Officer — the only agent authorized to finalize decisions.</summary>
    CEO = 1,

    /// <summary>Software developer responsible for implementing tasks and features.</summary>
    Developer = 2,

    /// <summary>Quality Assurance engineer — authorized to create and manage bug reports.</summary>
    QA = 3,

    /// <summary>Research agent responsible for gathering information and analysis.</summary>
    Researcher = 4,

    /// <summary>Documentation writer responsible for producing written artifacts.</summary>
    DocumentWriter = 5,

    /// <summary>Project manager coordinating tasks and workflows.</summary>
    ProjectManager = 6,

    /// <summary>DevOps engineer managing infrastructure and deployments.</summary>
    DevOps = 7,

    /// <summary>Security analyst responsible for identifying and mitigating risks.</summary>
    SecurityAnalyst = 8,

    /// <summary>Data analyst responsible for processing and interpreting data.</summary>
    DataAnalyst = 9,

    /// <summary>General-purpose agent with no specialized role.</summary>
    GeneralAgent = 10
}
