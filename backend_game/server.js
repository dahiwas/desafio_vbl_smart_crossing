const express = require("express");
const app = express();
const PORT = 3000;

const weathers = ["sunny", "clouded", "light rain", "heavy rain", "foggy", "stormy"];
// Tempos em ms em que as predicoes entram em vigor no jogo (5s, 10s, 18s, 30s)
const estimatedTimes = [5000, 10000, 18000, 30000];

function randomFloat(min, max, decimals = 1) {
  return parseFloat((Math.random() * (max - min) + min).toFixed(decimals));
}

// Gera 1-4 predicoes futuras com tempos aleatorios e ordenados cronologicamente.
// O jogo usa essas predicoes para agendar mudancas de trafego durante a partida.
function generatePredictions() {
  const count = Math.floor(Math.random() * 4) + 1;
  const times = [...estimatedTimes]
    .sort(() => Math.random() - 0.5)
    .slice(0, count)
    .sort((a, b) => a - b);

  return times.map((time) => ({
    estimated_time: time,
    predictions: {
      vehicleDensity: randomFloat(0.1, 1.0),
      averageSpeed: randomFloat(30, 90),
      weather: weathers[Math.floor(Math.random() * weathers.length)],
    },
  }));
}

app.get("/v1/traffic/status", (req, res) => {
  res.json({
    current_status: {
      vehicleDensity: randomFloat(0.1, 1.0),
      averageSpeed: randomFloat(30, 90),
      weather: weathers[Math.floor(Math.random() * weathers.length)],
    },
    predicted_status: generatePredictions(),
  });
});

app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
});
