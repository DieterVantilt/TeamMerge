// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Async void methods should be avoided unless they’re event handlers (or the logical equiv­alent of event handlers). Implementations of ICommand.Execute are logically event handlers and, thus, may be async void. See https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands", Scope = "member", Target = "~M:TeamMergeBase.Commands.AsyncRelayCommand`1.Execute(System.Object)")]
[assembly: SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Async void methods should be avoided unless they’re event handlers (or the logical equiv­alent of event handlers). Implementations of ICommand.Execute are logically event handlers and, thus, may be async void. See https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands", Scope = "member", Target = "~M:TeamMergeBase.Commands.AsyncRelayCommand.Execute(System.Object)")]
