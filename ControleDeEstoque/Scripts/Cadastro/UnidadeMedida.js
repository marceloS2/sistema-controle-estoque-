function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_sigla').val(dados.Sigla);
    $('#cbx_ativo').prop('checked', dados.Ativo); // aqui estão a criação de botões!

}

function set_focus_form() {
    $('#txt_nome').focus(); // esse função serve para quando abri o popup nao precida clikca dentro do label para escreve
}

function set_dados_grid(dados) {
    return '<td>' + dados.Nome + ' </td>' +
        '<td>' + dados.Sigla + ' </td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÂO') + '</td>'; // para criar se esta ativo ou nao!
}

function get_dados_inclusao() {
    return {
        Id: 0, Nome: '', Sigla:'', Ativo: true

    };

}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Sigla: $('#txt_sigla').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Sigla).end()
        .eq(2).html(param.Ativo ? 'SIM' : 'NÃO');
}