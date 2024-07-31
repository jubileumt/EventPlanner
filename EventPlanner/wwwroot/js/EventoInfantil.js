
// Alertas de salgados
function calcularMedia() {
    var salgados = document.getElementById('QuantSalgados').value;
    var pessoas = document.getElementById('QuantMaxPessoas').value;
    var criancas = document.getElementById('QuantCrian�as').value;

    if (salgados && pessoas && criancas) {
        var media = salgados / (parseInt(pessoas) + parseInt(criancas));
        var salgadosIdeal = 5.5;

        if (criancas > pessoas) {
            salgadosIdeal = 4.5;
        }

        var string = "A quantidade de salgados por pessoa �:"

        if (media < salgadosIdeal - 0.5) {
            alert(string + media + ', o que � muito pouco. O ideal seria cerca de ' + salgadosIdeal + ' salgados por pessoa.');
        } else if (media > salgadosIdeal + 0.5) {
            alert(string + media + ', o que � muito. O ideal seria cerca de ' + salgadosIdeal + ' salgados por pessoa.');
        } else {
            alert(string + media + ', o que � adequado.');
        }
    }
}
// Alertas de doces
document.getElementById('QuantDoces').addEventListener('change', calcularMediaDoces);
document.getElementById('QuantCriancas').addEventListener('change', calcularMediaDoces);

function calcularMediaDoces() {
    var Doces = document.getElementById('QuantDoces').value;
    var Pessoas = document.getElementById('QuantMaxPessoas').value;
    var Criancas = document.getElementById('QuantCriancas').value;

    if (Doces && Pessoas && Criancas) {
        var Media = (Doces / (parseInt(Pessoas) + parseInt(Criancas))).toFixed(2);
        var DocesIdeal = 2;

        if (Criancas > Pessoas) {
            DocesIdeal = 3;
        }

        var String = "A quantidade de doces por pessoa �:"

        if (Media < DocesIdeal - 0.5) {
            alert(String + Media + ', o que � muito pouco. O ideal seria cerca de ' + DocesIdeal + ' doces por pessoa.');
        } else if (Media > DocesIdeal + 0.5) {
            alert(String + Media + ', o que � muito. O ideal seria cerca de ' + DocesIdeal + ' doces por pessoa.');
        } else {
            alert(String + Media + ', o que � adequado.');
        }
    }
}



// Alerta refrigerante
document.getElementById('QuantMaxPessoas').addEventListener('change', calcularRefriPorPessoa);
document.getElementById('QuantRefri').addEventListener('change', calcularRefriPorPessoa);
document.getElementById('QuantCriancas').addEventListener('change', calcularRefriPorPessoa);
function calcularRefriPorPessoa() {
    var pessoas = document.getElementById('QuantMaxPessoas').value;
    var refri = document.getElementById('QuantRefri').value;
    var criancas = document.getElementById('QuantCriancas').value;

    if (pessoas && refri && criancas) {
        var refriPorPessoa = refri / (parseInt(pessoas) + parseInt(criancas));
        var mensagem = 'A quantidade m�dia de refrigerante por pessoa � ' + refriPorPessoa.toFixed(2) + ' ml. ';

        var refriIdeal = 200;

        if (criancas > pessoas) {
            refriIdeal = 150;
        }

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

//Alertas cadeiras
document.getElementById('QuantMaxPessoas').addEventListener('change', verificarCadeiras);
document.getElementById('QuantCadeiras').addEventListener('change', verificarCadeiras);
function verificarCadeiras() {
    var pessoas = document.getElementById('QuantMaxPessoas').value;
    var cadeiras = document.getElementById('QuantCadeiras').value;

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
document.getElementById('DataInicio').addEventListener('change', function () {

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
document.getElementById('QuantMaxPessoas').addEventListener('change', verificarEspaco);
document.getElementById('QuantMetrosQuadrados').addEventListener('change', verificarEspaco);
function verificarEspaco() {
    var pessoas = document.getElementById('QuantMaxPessoas').value;
    var espaco = document.getElementById('QuantMetrosQuadrados').value;

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

document.getElementById('CEP').addEventListener('change', function () {
    var cep = this.value.replace(/\D/g, '');

    if (cep != "") {
        var validacep = /^[0-9]{8}$/;

        if (validacep.test(cep)) {
            fetch(`https://viacep.com.br/ws/${cep}/json/`)
                .then(response => response.json())
                .then(data => {
                    if (!("erro" in data)) {
                        document.getElementById('Bairro').value = data.bairro;
                        document.getElementById('Cidade').value = data.localidade;
                        document.getElementById('Estado').value = data.uf;
                    } else {
                        alert("CEP n�o encontrado.");
                    }
                })
                .catch(() => alert("Erro ao buscar CEP."));
        } else {
            alert("Formato de CEP inv�lido.");
        }
    }
});

