//Api do clima
window.onload = function () {
    var API_KEY = '30c78641fc01738908a99eec902e61ae';
    var cidade = 'Salvador,BR';

    var url = 'https://api.openweathermap.org/data/2.5/weather?q=' + cidade + '&appid=' + API_KEY + '&lang=pt_br';

    fetch(url)
        .then(response => response.json())
        .then(data => {

            var descricao = data.weather[0].description;
            var temperatura = (data.main.temp - 273.15).toFixed(2);

            document.getElementById('descricao').textContent = descricao;
            document.getElementById('temperatura').textContent = temperatura + ' graus C';
        })
        .catch(error => console.error('Erro:', error));
};

document.getElementById("CEP").onchange = function () {
    var cep = document.getElementById("CEP").value;
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://viacep.com.br/ws/" + cep + "/json/");
    xhr.onload = function () {
        var response = JSON.parse(xhr.responseText);
        document.getElementById("Bairro").value = response.bairro;
        document.getElementById("Cidade").value = response.localidade;
        document.getElementById("Estado").value = response.uf;
    };
    xhr.send();
};