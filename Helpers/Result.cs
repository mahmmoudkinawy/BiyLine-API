﻿namespace BiyLineApi.Helpers;
public sealed class Result<T> where T : class
{
    public bool IsSuccess { get; set; }
    public bool IsBadRequest { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }
    public List<string> Errors { get; set; }

    public int StatusCode { get; set; }
    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(List<string> errors) =>
        new() { IsSuccess = false, Errors = errors };
    public static Result<T> Failure(string error) =>
        new() { IsSuccess = false, Error = error };
    public static Result<T> BadRequest(string error) =>
        new() { IsBadRequest = true, Error = error, StatusCode = 400 };
     public static Result<T> BadRequest(List<string> errors) =>
        new() { IsBadRequest = true, Errors = errors, StatusCode = 400 };

}
