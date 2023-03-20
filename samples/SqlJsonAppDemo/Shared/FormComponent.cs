// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Telerik.Blazor.Components;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace SqlJsonAppDemo.Shared;

/// <summary>
/// 
/// </summary>
public class FormComponent : ComponentBase
{
    [Inject]
    protected ICrudDbService db { get; set; }

    [Inject]
    protected IDbLogService dbLog { get; set; }

    [Parameter]
    public string? Title { get; set; }

    protected string? ErrMsg { get; set; }

    protected bool Loading { get; set; }

    protected int PageSize { get; set; } = 100;
    protected List<int?> PageSizes = new() { 10, 25, 50, 100, null };

    protected string? WinTitle { get; set; }
    protected bool WindowVisible { get { return !string.IsNullOrEmpty(WinTitle); } set { if (!value) WinTitle = null; } }

    protected virtual object? ID { get; }

    public virtual async Task BindData(string? settings, string? values = default) { await Task.CompletedTask; }

    protected virtual async Task InitData() { await Task.CompletedTask; }
    protected virtual async Task LoadData() { await Task.CompletedTask; }
    protected virtual async Task GetItemAsync(GridCommandEventArgs args) { await Task.CompletedTask; }
    protected virtual async Task SetItemAsync(EditContext editContext) { await Task.CompletedTask; }
    protected virtual async Task InsItemAsync(GridCommandEventArgs args) { await Task.CompletedTask; }
    protected virtual async Task UpdItemAsync(GridCommandEventArgs args) { await Task.CompletedTask; }
    protected virtual async Task DelItemAsync(GridCommandEventArgs args) { await Task.CompletedTask; }

    protected override async Task OnInitializedAsync()
    {
        ErrMsg = null; Loading = true;
        try
        {
            await InitData();

            await LoadData();
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            await dbLog.LogErrorAsync($"{Title}/OnInitializedAsync", new { ex.Message, ex.StackTrace });
        }
        Loading = false;
    }

    protected async Task RefreshHandler()
    {
        ErrMsg = null; Loading = true;
        try
        {
            await LoadData();
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            await dbLog.LogErrorAsync($"{Title}/RefreshHandler", new { ex.Message, ex.StackTrace });
        }
        Loading = false;
    }

    protected async Task CreateHandler(GridCommandEventArgs args)
    {
        ErrMsg = null; Loading = true;
        try
        {
            await InsItemAsync(args);

            await LoadData();

            ErrMsg = "Created successfully.";
            await dbLog.LogInformationAsync($"{Title}/CreateHandler", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            await dbLog.LogErrorAsync($"{Title}/CreateHandler", new { ex.Message, ex.StackTrace });
            args.IsCancelled = true;
        }
        Loading = false;
    }

    protected async Task UpdateHandler(GridCommandEventArgs args)
    {
        ErrMsg = null; Loading = true;
        try
        {
            await UpdItemAsync(args);

            await LoadData();

            ErrMsg = "Updated successfully.";
            await dbLog.LogInformationAsync($"{Title}/UpdateHandler/{ID}", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            await dbLog.LogErrorAsync($"{Title}/UpdateHandler/{ID}", new { ex.Message, ex.StackTrace });
            args.IsCancelled = true;
        }
        Loading = false;
    }

    protected async Task DeleteHandler(GridCommandEventArgs args)
    {
        ErrMsg = null; Loading = true;
        try
        {
            await DelItemAsync(args);

            await LoadData();

            ErrMsg = "Deleted successfully.";
            await dbLog.LogInformationAsync($"{Title}/DeleteHandler/{ID}", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            await dbLog.LogErrorAsync($"{Title}/DeleteHandler/{ID}", new { ex.Message, ex.StackTrace });
        }
        Loading = false;
    }

    protected async Task SubmitHandler(EditContext editContext)
    {
        ErrMsg = null; Loading = true;
        if (editContext.Validate())
        {
            await SetItemAsync(editContext);

            ErrMsg = "Submit successfully.";
            await dbLog.LogInformationAsync($"{Title}/SubmitHandler/{ID}", ErrMsg);
        }
        else
        {
            ErrMsg = $"Error: Invalid Data. Submit canceled.";
            await dbLog.LogErrorAsync($"{Title}/SubmitHandler/{ID}", ErrMsg);
        }
        Loading = false;
    }
}
