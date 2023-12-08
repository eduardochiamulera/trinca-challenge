using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System;

namespace CrossCutting.Models
{
	public class ServiceResult<T>
	{
		private ServiceResult(bool isSuccess, Error error)
		{
			if (isSuccess && error != null ||
				!isSuccess && error == null)
			{
				throw new ArgumentException("Invalid error", nameof(error));
			}

			IsSuccess = isSuccess;
			Error = error;
		}

		private ServiceResult(bool isSuccess, T data, string message = "")
		{
			IsSuccess = isSuccess;
			Data = data;
			Message = string.IsNullOrWhiteSpace(message) ? "Operation successfully completed." : message;
		}


		public T Data { get; }
		public Error Error { get; }
		public bool IsSuccess { get; }
		public string Message { get; }	

		public static ServiceResult<T> Success(T data, string message = "") => new ServiceResult<T>(true, data, message);

		public static ServiceResult<T> Failure(Error error) => new ServiceResult<T>(false, error);
	}
}
