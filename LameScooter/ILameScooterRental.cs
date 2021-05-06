using System.Threading.Tasks;

namespace LameScooter {

    public interface ILameScooterRental {
        string Method { get; }
        Task<int> GetScooterCountInStation(string stationName);
    }

}