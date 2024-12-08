import http from 'k6/http';
function getRandomInt(max) {
  return Math.floor(Math.random() * max);
}

var getCar = function () {
  var cars = ["Volkswagen Polo", "Citroen C4", "Renault Clio"];
  return cars[getRandomInt(3)];
};

var getUser = function () {
  var cars = ["Isaac Asimov", "Dorian Gray", "Nikola Tesla"];
  return cars[getRandomInt(3)];
};

export default function () {
  const url = 'http://localhost:5001/bid';
  const payload = JSON.stringify(
    {
      "car": getCar(),
      "user": getUser(),
      "value": getRandomInt(10000)
    }
  );

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  http.post(url, payload, params);
}