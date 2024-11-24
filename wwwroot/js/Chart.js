

// Grafik verilerini oluşturuyoruz
var ctx = document.getElementById('myChart').getContext('2d');
var chart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: pieChartData.map(function (data) { return data.CategoryName; }),
        datasets: [{
            label: 'Harcamalar',
            data: pieChartData.map(function (data) { return data.Amount; }),
            backgroundColor: [
                'rgba(255, 0, 0, 0.7)',   // Kırmızı
                'rgba(0, 0, 255, 0.7)',   // Mavi
                'rgba(255, 165, 0, 0.7)', // Turuncu
                'rgba(0, 255, 0, 0.7)',   // Yeşil
                'rgba(128, 0, 128, 0.7)', // Mor
                'rgba(255, 255, 0, 0.7)'  // Sarı
            ],
            borderColor: [
                'rgba(255, 0, 0, 1)',     // Kırmızı
                'rgba(0, 0, 255, 1)',     // Mavi
                'rgba(255, 165, 0, 1)',   // Turuncu
                'rgba(0, 255, 0, 1)',     // Yeşil
                'rgba(128, 0, 128, 1)',   // Mor
                'rgba(255, 255, 0, 1)'    // Sarı
            ],
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false, // Oranı korumamak için
        plugins: {
            legend: {
                position: 'top',
            },
            tooltip: {
                callbacks: {
                    label: function (tooltipItem) {
                        return tooltipItem.label + ': ' + tooltipItem.raw.toLocaleString();
                    }
                }
            }
        }
    }
});