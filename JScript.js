// Archivo JScript

function abrirpagina(pag){

  window.open(pag,"_self" ,"status=yes,toolbar=yes,menubar=yes,location=no,height=630px,width=810px");


}
function popUpReport(url, parametros)
{
    window.open(url+"?p="+parametros, "CustomPopUp","width=800, height=450 , menubar=yes ,toolbar=yes,scrollbars=yes, resizable=yes, location=yes");
    
}