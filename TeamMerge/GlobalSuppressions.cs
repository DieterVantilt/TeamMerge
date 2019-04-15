
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Necessary for long running threads so UI doesn't freeze", Scope = "member", Target = "~M:TeamMerge.Commands.AsyncRelayCommand.Execute(System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Necessary for long running threads so UI doesn't freeze", Scope = "member", Target = "~M:TeamMerge.Commands.AsyncRelayCommand`1.Execute(System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Will never be null", Scope = "member", Target = "~M:TeamMerge.Services.TeamExplorerServiceVS2017.#ctor(System.IServiceProvider,Logic.Services.ITFVCService)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Will never be null", Scope = "member", Target = "~M:LogicVS2019.Services.TeamExplorerServiceVS2019.#ctor(System.IServiceProvider,Logic.Services.ITFVCService)")]