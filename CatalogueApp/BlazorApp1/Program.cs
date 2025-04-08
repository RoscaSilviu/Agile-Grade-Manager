using BlazorApp1.Components.Services;
using Blazored.LocalStorage;
using CatalogueApp.Components;
using CatalogueApp.Components.Models;
using CatalogueApp.Components.Pages;
using CatalogueApp.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddServerSideBlazor().AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<GradeDashboardService>();
builder.Services.AddScoped<GradeHistoryService>();
builder.Services.AddScoped<TeacherGradeHistoryService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7054/") });

builder.Services.AddScoped<TeacherMainPage>();
builder.Services.AddScoped<ClassModel>();

builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<AssignmentService>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
