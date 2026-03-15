using System;

namespace Arpg.Client.Core;

public static class ApiEndpoints
{
    private const string BaseApi = "api";

    public static class User
    {
        public const string Login = $"{BaseApi}/{nameof(User)}/login";
        public const string New = $"{BaseApi}/{nameof(User)}";
        public const string GetSelf = $"{BaseApi}/{nameof(User)}";
        public static string GetFlat(string username) => $"{BaseApi}/{nameof(User)}/{username}";
        public const string Delete = $"{BaseApi}/{nameof(User)}";
        public const string Edit = $"{BaseApi}/{nameof(User)}";
    }

    public static class Template
    {
        public const string Create = $"{BaseApi}/{nameof(Template)}";
        public static string Get(Guid id) => $"{BaseApi}/{nameof(Template)}/{id}";
        public const string GetList = $"{BaseApi}/{nameof(Template)}";
        public const string Edit = $"{BaseApi}/{nameof(Template)}";
        public const string Delete = $"{BaseApi}/{nameof(Template)}";
    }

    public static class Structure
    {
        public static string StructureUpdate(Guid templateId) => $"{BaseApi}/{nameof(Structure)}/{templateId}/structure";
    }

    public static class Sheet
    {
        public const string Create = $"{BaseApi}/{nameof(Sheet)}";
        public static string Get(Guid id) => $"{BaseApi}/{nameof(Sheet)}/{id}";
        public const string GetList = $"{BaseApi}/{nameof(Sheet)}";
        public const string Edit = $"{BaseApi}/{nameof(Sheet)}";
        public static string Update(Guid id) => $"{BaseApi}/{nameof(Sheet)}/{id}";
        public static string Delete(Guid id) => $"{BaseApi}/{nameof(Sheet)}/{id}";
    }

    public static class ExpressionResolver
    {
        public const string Resolve = $"{BaseApi}/{nameof(ExpressionResolver)}/resolve";
    }
}
