﻿function add_anti_forgery_token(data) { // evitando atack CSRF
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_mensagem_aviso(mensagens) {
    var ret = '';
    for (var i = 0; i < mensagens.length; i++) {
        ret += '<li>' + mensagens[i] + '</li>';
    }
    return '<ul>' + ret + '</ul>';

}
function abrir_form(dados) {
    set_dados_form(dados);
   
    var modal_cadastro = $('#modal_cadastro');
    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide(); //deixando escodindo

    bootbox.dialog({ // bootbox edit
        title: ' Cadastro ' + tituloPagina, //  é referente o titulo que esta la no começo
        message: modal_cadastro

    })

        .on('shown.bs.modal', function () {
            modal_cadastro.show(0, function () {
                set_focus_form();
                
            });
        })

        .on('hidden.bs.modal', function () {
            modal_cadastro.hide().appendTo('body');

        });
}

function criar_linha_grid(dados) {
    var ret =
        '<tr data-id=' + dados.Id + '>' +
        set_dados_grid(dados) +
        '<td>' +
        '<a class="btn btn-primary btn-alterar" role="button"><i class="glyphicon glyphicon-pencil"></i> Alterar</a>' +
        '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>' +
        '</td>' +
        '</tr>';

    return ret;
}


$(document).on('click', '#btn_incluir', function () {

    abrir_form(get_dados_inclusao()); //mandando um parametro para aprecer no botao incluir o nome e se esta ativo
})
    .on('click', '.btn-alterar', function () {
        var btn = $(this),
            id = btn.closest('tr').attr('data-id'),
            url = url_alterar,
            param = { 'id': id };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                abrir_form(response);

            }

        });

    })
    .on('click', '.btn-excluir', function () {
        var btn = $(this),
            tr = btn.closest('tr'),
            id = tr.attr('data-id'),
            url = url_excluir,
            param = { 'id': id };

        bootbox.confirm({
            message: "Realmente deseja excluir o " + tituloPagina + "?",
            buttons: {
                confirm: {
                    label: 'Sim',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'Não',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post(url, add_anti_forgery_token(param), function (response) {
                        if (response) {
                            tr.remove();
                            var quant = $('#grid_cadastro > tbody > tr').length;
                            if (quant == 0) {
                                $('#mensagem_grid').removeClass('display-none'); // aqui é a logica onde aparcer a massagem que não existe registro quando remove
                                $('#grid_cadastro').addClass('display-none');
                            }
                        }
                    });
                }
            }
        });
    })

    .on('click', '#btn_confirmar', function () {
        var btn = $(this),
            url = url_confirmar,
            param = get_dados_form(); // esta vindo la de cima getdados

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response.Resultado == "OK") {
                if (param.Id == 0) {
                    param.Id = response.IdSalvo;
                    var table = $('#grid_cadastro').find('tbody'),
                        linha = criar_linha_grid(param);
                    table.append(linha);


                    $('#grid_cadastro').removeClass('display-none');
                    $('#mensagem_grid').addClass('display-none'); // aqui é a logica onde aparcer a massagem que não existe registro quando adciona
                }
                else {
                    var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
                    preencher_linha_grid(param, linha); // esta vindo la de cima linha_grid
                 

                }
                $('#modal_cadastro').parents('.bootbox').modal('hide');
            }
            else if (response.Resultado == "ERRO") {
                $('#msg_aviso').hide();
                $('#msg_mensagem_aviso').hide();
                $('#msg_erro').show();
            }
            else if (response.Resultado == "AVISO") {
                $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
                $('#msg_aviso').show();
                $('#msg_mensagem_aviso').show();
                $('#msg_erro').hide();
            }
        });

    })
    .on('click', '.page-item', function () { // craindo a parte 2 da pagina grid

        var btn = $(this),
            filtro = $('#txt_filtro'),
            tamPag = $('#ddl_tam_pag').val(),
            pagina = btn.text(),
            url = url_page_click,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val()};

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                btn.siblings().removeClass('active');
                btn.addClass('active');
            }
        });
    })
    .on('change', '#ddl_tam_pag', function () {
        var ddl = $(this),
            filtro = $('#txt_filtro'),
            tamPag = ddl.val(), // configuração de pagina do dropdownselect
            pagina = 1,
            url = url_tam_pag_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val()};

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        });
    })
    .on('keyup', '#txt_filtro', function () { //evento do botão de pesquisa
        var filtro = $(this),
            ddl = $('#ddl_tam_pag'),
            tamPag = ddl.val(), // configuração de pagina do dropdownselect
            pagina = 1,
            url = url_filtro_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val()};

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        });
    });