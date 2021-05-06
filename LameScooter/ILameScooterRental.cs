using System.Threading.Tasks;

namespace LameScooter {

    public interface ILameScooterRental {
        Task<int> GetScooterCountInStation(string stationName);
    }

}