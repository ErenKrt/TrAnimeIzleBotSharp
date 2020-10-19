using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes
{
    public interface IResult<out T>
    {
        bool Succeeded { get; }
        T Value { get; }
        string Info { get; }
    }
    public class Result<T>: IResult<T>
    {
        public Result(bool succeeded, T value, string info)
        {
            Succeeded = succeeded;
            Value = value;
            Info = info;
        }
        public Result(bool succeeded, string info)
        {
            Succeeded = succeeded;
            Info = info;
        }

        public Result(bool succeeded, T value)
        {
            Succeeded = succeeded;
            Value = value;
        }

        public bool Succeeded { get; }
        public T Value { get; }
        public string Info { get; } 
    }
    public static class Result
    {
        public static IResult<T> Success<T>(T resValue)
        {
            return new Result<T>(true, resValue, "No errors");
        }

        public static IResult<T> Success<T>(string successMsg, T resValue)
        {
            return new Result<T>(true, resValue, successMsg);
        }

        public static IResult<T> Fail<T>(Exception exception)
        {
            return new Result<T>(false, default(T), exception.Message);
        }

        public static IResult<T> Fail<T>(string errMsg)
        {
            return new Result<T>(false, default(T), errMsg);
        }

        public static IResult<T> Fail<T>(string errMsg, T resValue)
        {
            return new Result<T>(false, resValue, errMsg);
        }


    }

}
