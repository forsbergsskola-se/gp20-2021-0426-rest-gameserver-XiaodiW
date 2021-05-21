using System.Threading.Tasks;

namespace LameScooter.Interface {

    public interface ILameScooterRental {
        string Method { get; }
        Task<int> GetScooterCountInStation(string stationName);
    }

}