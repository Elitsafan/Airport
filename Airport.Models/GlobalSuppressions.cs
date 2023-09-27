﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IAirportService.Start~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IAirportService.GetSummary~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IAirportService.GetStatus~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IRouteLogic.GetRightOfWay(Airport.Models.Interfaces.IStationLogic,Airport.Models.Interfaces.IStationLogic,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.VisualStudio.Threading.AsyncSemaphore.Releaser}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IRouteLogic.StartRun~System.Threading.Tasks.Task{Microsoft.VisualStudio.Threading.AsyncSemaphore.Releaser}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IStationLogic.SetFlight(Airport.Models.Interfaces.IFlightLogic,System.Threading.CancellationTokenSource)~System.Threading.Tasks.Task{Airport.Models.Interfaces.IStationLogic}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IStationLogic.Clear~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLogic.Run~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLogic.ThrowIfCancellationRequested(System.Threading.CancellationTokenSource)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLauncherService.Start~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLauncherService.LaunchMany(System.String[])~System.Collections.Generic.IAsyncEnumerable{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLauncherService.LaunchMany~System.Collections.Generic.IAsyncEnumerable{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLauncherService.SetFlightTimeout(System.Nullable{Airport.Models.Enums.FlightType})~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "member", Target = "~M:Airport.Models.Interfaces.IFlightLauncherService.LaunchOne(Airport.Models.Enums.FlightType)~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]
