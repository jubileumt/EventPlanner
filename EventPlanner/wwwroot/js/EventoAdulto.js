// M�todo para calcular a m�dia de salgados por pessoa
document.getElementById('QuantSalgadosAdulto').addEventListener('change', calcularMediaSalgadosAdulto);
document.getElementById('QuantMaxPessoasAdulto').addEventListener('change', calcularMediaSalgadosAdulto);

function calcularMediaSalgadosAdulto() {
    var salgados = document.getElementById('QuantSalgadosAdulto').value;
    var pessoas = document.getElementById('QuantMaxPessoasAdulto').value;

    if (salgados && pessoas) {
        var media = salgados / pessoas;
        var salgadosIdeal = 6.5; // Ajuste a quantidade ideal de salgados por pessoa aqui

        var string = "A quantidade de salgados por pessoa e:"

        if (media < salgadosIdeal - 0.5) {
            alert(string + media + ', o que e muito pouco. O ideal seria cerca de ' + salgadosIdeal + ' salgados por pessoa.');
        } else if (media > salgadosIdeal + 0.5) {
            alert(string + media + ', o que � muito. O ideal seria cerca de ' + salgadosIdeal + ' salgados por pessoa.');
        } else {
            alert(string + media + ', o que � adequado.');
        }
    }
}

document.getElementById('QuantDocesAdulto').addEventListener('change', calcularMediaDocesAdulto);
document.getElementById('QuantMaxPessoasAdulto').addEventListener('change', calcularMediaDocesAdulto);

function calcularMediaDocesAdulto() {
    var doces = document.getElementById('QuantDocesAdulto').value;
    var pessoas = document.getElementById('QuantMaxPessoasAdulto').value;

    if (doces && pessoas) {
        var media = doces / pessoas;
        var docesIdeal = 1.25;

        var string = "A quantidade de doces por pessoa �:"

        if (media < docesIdeal - 0.25) {
            alert(string + media + ', o que � muito pouco. O ideal seria cerca de ' + docesIdeal + ' doces por pessoa.');
        } else if (media > docesIdeal + 0.25) {
            alert(string + media + ', o que � muito. O ideal seria cerca de ' + docesIdeal + ' doces por pessoa.');
        } else {
            alert(string + media + ', o que � adequado.');
        }
    }
}

// M�todo para calcular a m�dia de refrigerante por pessoa
document.getElementById('QuantRefriAdulto').addEventListener('change', calcularRefriPorPessoaAdulto);
document.getElementById('QuantMaxPessoasAdulto').addEventListener('change', calcularRefriPorPessoaAdulto);
function calcularRefriPorPessoaAdulto() {
    var pessoas = document.getElementById('QuantMaxPessoasAdulto').value;
    var refri = document.getElementById('QuantRefriAdulto').value;

    if (pessoas && refri) {
        var refriPorPessoa = refri / pessoas;
        var mensagem = 'A quantidade m�dia de refrigerante por pessoa � ' + refriPorPessoa.toFixed(2) + ' ml. ';

        var refriIdeal = 200;

        if (refriPorPessoa > refriIdeal + 50) {
            mensagem += 'Isso parece ser uma quantidade alta de refrigerante por pessoa.';
        } else if (refriPorPessoa < refriIdeal - 50) {
            mensagem += 'Isso parece ser uma quantidade baixa de refrigerante por pessoa.';
        } else {
            mensagem += 'Isso parece ser uma quantidade adequada de refrigerante por pessoa.';
        }

        alert(mensagem);
    }
}

// Alertas de cadeiras
document.getElementById('QuantMaxPessoasAdulto').addEventListener('change', verificarCadeiras);
document.getElementById('QuantCadeirasAdulto').addEventListener('change', verificarCadeiras);

function verificarCadeiras() {
    var pessoas = document.getElementById('QuantMaxPessoasAdulto').value;
    var cadeiras = document.getElementById('QuantCadeirasAdulto').value;

    if (pessoas && cadeiras) {
        if (pessoas > cadeiras) {
            var emPe = pessoas - cadeiras;
            alert('H� mais pessoas do que cadeiras! ' + emPe + ' pessoas ficar�o em p�.');
        } else if (pessoas < cadeiras) {
            var sobrando = cadeiras - pessoas;
            alert('H� mais cadeiras do que pessoas! ' + sobrando + ' cadeiras estar�o vazias.');
        } else {
            alert('A quantidade de pessoas � igual � quantidade de cadeiras.');
        }
    }
}

//Alertas de data
document.getElementById('DataInicioAdulto').addEventListener('change', function () {

    if (this.readOnly) {
        return;
    }

    var dataInicio = new Date(this.value);
    var dataAtual = new Date();

    var diferenca = Math.ceil((dataInicio - dataAtual) / (1000 * 60 * 60 * 24));

    if (diferenca < 30) {
        alert('A data de in�cio � menos de 30 dias a partir de hoje.');
    }
});


// Alertas de espaco
document.getElementById('QuantMaxPessoasAdulto').addEventListener('change', verificarEspaco);
document.getElementById('QuantMetrosQuadradosAdulto').addEventListener('change', verificarEspaco);
function verificarEspaco() {
    var pessoas = document.getElementById('QuantMaxPessoasAdulto').value;
    var espaco = document.getElementById('QuantMetrosQuadradosAdulto').value;

    if (pessoas && espaco) {
        var densidade = pessoas / espaco;

        if (densidade > 4) {
            alert('A quantidade de pessoas � muito grande para o espa�o dispon�vel! A densidade � ' + densidade.toFixed(2) + ' pessoas por metro quadrado.');
        } else if (densidade > 3) {
            alert('O espa�o pode ficar um pouco apertado. A densidade � ' + densidade.toFixed(2) + ' pessoas por metro quadrado.');
        } else if (densidade > 1) {
            alert('A quantidade de pessoas � adequada para o espa�o dispon�vel. A densidade � ' + densidade.toFixed(2) + ' pessoas por metro quadrado.');
        } else if (densidade > 0.5) {
            alert('O espa�o dispon�vel � grande para a quantidade de pessoas. A densidade � ' + densidade.toFixed(2) + ' pessoas por metro quadrado.');
        } else {
            alert('O espa�o dispon�vel � muito grande para a quantidade de pessoas! A densidade � ' + densidade.toFixed(2) + ' pessoas por metro quadrado.');
        }
    }
}


//Api do CEP
document.getElementById("CEPAdulto").onchange = function () {
    var cep = document.getElementById("CEPAdulto").value;
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://viacep.com.br/ws/" + cep + "/json/");
    xhr.onload = function () {
        var response = JSON.parse(xhr.responseText);
        document.getElementById("BairroAdulto").value = response.bairro;
        document.getElementById("CidadeAdulto").value = response.localidade;
        document.getElementById("EstadoAdulto").value = response.uf;
    };
    xhr.send();
};