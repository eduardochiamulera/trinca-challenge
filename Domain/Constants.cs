using CrossCutting.Models;

namespace Domain
{
	public static class Constants
	{
		public const int NumeroConfirmacoesAlteraStatusBbq = 7;
		public const decimal QuantidadeCarneKilos = 0.3m;
		public const decimal QuantidadeVegetaisVegetarianosKilos = 0.6m;
		public const decimal QuantidadeVegetaisKilos = 0.3m;

		public static Error InputRequired => BadRequest("Input is required");
		public static Error BadRequest(string message) => new Error(400, message);
		public static Error NotFound(string value) => new Error(404, $"{value} not found.");
	}
}
