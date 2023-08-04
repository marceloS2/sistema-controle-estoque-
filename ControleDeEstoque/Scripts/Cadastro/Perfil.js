function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#cbx_ativo').prop('checked', dados.Ativo); // aqui estão a criação de botões!

    var lista_usuario = $('#lista_usuario');
    lista_usuario.find('input[type=checkbox]').prop('checked', false); //pegadno todos os ipunt do tipo checkbox

    if (dados.Usuarios) {
        for (var i = 0; i < dados.Usuarios.length; i++) {
            var usuario = dados.Usuarios[i];
            var cbx = lista_usuario.find('input[data-id-usuario=' + usuario.Id + ']');
            if (cbx)
            {
                cbx.prop('checked', true);
            }
        }
    }
}
function set_focus_form() {
    $('#txt_nome').focus(); // esse função serve para quando abri o popup nao precida clikca dentro do label para escreve
}

function set_dados_grid(dados) {
    return '<td>' + dados.Nome + ' </td>' + // para criar se esta ativo ou nao!
        '<td>' + (dados.Ativo ? 'SIM' : 'NÂO') + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0, Nome: '', Ativo: true

    };

}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
}

