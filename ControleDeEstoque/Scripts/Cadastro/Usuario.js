function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_login').val(dados.Login);
    $('#txt_senha').val(dados.Senha);
   
}

function set_focus_form() {
    $('#txt_nome').focus(); // esse função serve para quando abri o popup nao precida clikca dentro do label para escreve
}

function set_dados_grid(dados) {
    return '<td>' + dados.Nome + ' </td>' + // para criar se esta ativo ou nao!
        '<td>' + dados.Login + ' </td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Login: '',
        Senha: '' //mandando um parametro no forme pra quando ele abri embranco
       
    };

}

function get_dados_form() {
   return{
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Login: $('#txt_login').val(),
       senha: $('#txt_senha').val()
       

    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Login);
}
