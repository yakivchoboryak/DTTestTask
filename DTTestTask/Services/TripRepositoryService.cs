using DTTestTask.Models;
using EFCore.BulkExtensions;

namespace DTTestTask.Services
{
    public class TripRepositoryService
    {
        /// <summary>
        /// TODO:
        /// implement Batch Processing
        /// mb Parallel Processing
        /// </summary>
        /// <param name="tripContext"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task InsertCsvDataToDatabaseAsync(TripContext tripContext, Stream stream)
        {
            try
            {
                var uniqueTrips = new Dictionary<string, Trip>();
                using var reader = new StreamReader(stream);
                await reader.ReadLineAsync(); // Skip header line

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line!.Split(',');

                    try
                    {
                        string key = $"{values[1]}_{values[2]}_{values[3]}";

                        if (!uniqueTrips.ContainsKey(key))
                        {
                            var trip = ParseTrip(values);
                            uniqueTrips.Add(key, trip);
                        }
                    }
                    catch (Exception)
                    {
                        // Skip invalid lines
                        continue;
                    }
                }

                var uniqueTripList = uniqueTrips.Values.ToList();
                await WriteDuplicatesToFileAsync(uniqueTripList, tripContext);

                await tripContext.BulkInsertAsync<Trip>(uniqueTripList);
                await tripContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Trip ParseTrip(string[] values)
        {
            return new Trip
            {
                PickupDateTime = DateTime.Parse(values[1]),
                DropoffDateTime = DateTime.Parse(values[2]),
                PassengerCount = int.Parse(values[3]),
                TripDistance = double.Parse(values[4]),
                StoreAndForwardFlag = values[6] == "Y" ? "Yes" : "No",
                PickupLocationID = int.Parse(values[8]),
                DropoffLocationID = int.Parse(values[9]),
                FareAmount = decimal.Parse(values[11]),
                TipAmount = decimal.Parse(values[14])
            };
        }


        private async Task WriteDuplicatesToFileAsync(List<Trip> trips, TripContext tripContext)
        {
            var duplicates = trips.GroupBy(t => new { t.PickupDateTime, t.DropoffDateTime, t.PassengerCount })
                                  .Where(g => g.Count() > 1)
                                  .SelectMany(g => g);

            using (StreamWriter writer = new StreamWriter("duplicates.csv"))
            {
                foreach (var duplicate in duplicates)
                {
                    await writer.WriteLineAsync($"{duplicate.PickupDateTime},{duplicate.DropoffDateTime},{duplicate.PassengerCount}");
                }
            }
        }
    }
}
