// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Telerik.Blazor.Components;

namespace BlazorWasmDemo.Client.Shared;

/// <summary>
/// 
/// </summary>
public class FormComponent : HttpComponent
{
    [Parameter]
    public string? Title { get; set; }

    protected string? ErrMsg { get; set; }

    protected bool Loading { get; set; }

    protected int PageSize { get; set; } = 100;
    protected List<int?> PageSizes = new() { 10, 25, 50, 100, null };

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
            // await LogHelper.LogErrorAsync($"{Title}/OnInitializedAsync", new { ex.Message, ex.StackTrace });
        }
        Loading = false;
    }

    protected async Task Refresh()
    {
        ErrMsg = null;
        try
        {
            await LoadData();
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            //await LogHelper.LogErrorAsync($"{Title}/Refresh", new { ex.Message, ex.StackTrace });
        }
    }

    protected async Task CreateHandler(GridCommandEventArgs args)
    {
        ErrMsg = null;
        try
        {
            await InsItemAsync(args);

            await LoadData();

            ErrMsg = "Created successfully.";
            //await LogHelper.LogInformationAsync($"{Title}/CreateHandler", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            //await LogHelper.LogErrorAsync($"{Title}/CreateHandler", new { ex.Message, ex.StackTrace });
            args.IsCancelled = true;
        }
    }

    protected async Task UpdateHandler(GridCommandEventArgs args)
    {
        ErrMsg = null;
        try
        {
            await UpdItemAsync(args);

            await LoadData();

            ErrMsg = "Updated successfully.";
            //await LogHelper.LogInformationAsync($"{Title}/UpdateHandler/{ID}", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            //await LogHelper.LogErrorAsync($"{Title}/UpdateHandler/{ID}", new { ex.Message, ex.StackTrace });
            args.IsCancelled = true;
        }
    }

    protected async Task DeleteHandler(GridCommandEventArgs args)
    {
        ErrMsg = null;
        try
        {
            await DelItemAsync(args);

            await LoadData();

            ErrMsg = "Deleted successfully.";
            //await LogHelper.LogInformationAsync($"{Title}/DeleteHandler/{ID}", ErrMsg);
        }
        catch (Exception ex)
        {
            ErrMsg = $"Error: {ex.Message}";
            //await LogHelper.LogErrorAsync($"{Title}/DeleteHandler/{ID}", new { ex.Message, ex.StackTrace });
        }
    }

    protected async Task SubmitHandler(EditContext editContext)
    {
        ErrMsg = null;
        if (editContext.Validate())
        {
            await SetItemAsync(editContext);

            ErrMsg = "Submit successfully.";
            //await LogHelper.LogInformationAsync($"{Title}/SubmitHandler/{ID}", ErrMsg);
        }
        else
        {
            ErrMsg = $"Error: Invalid Data. Submit canceled.";
            //await LogHelper.LogErrorAsync($"{Title}/SubmitHandler/{ID}", ErrMsg);
        }
    }
}
