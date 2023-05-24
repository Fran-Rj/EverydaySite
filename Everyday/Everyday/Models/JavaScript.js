// MOSTRAR IMAGEN
function mostrarimagen(input) {
    if (input.files && input.files[0]) {
        var leer = new FileReader();
        leer.onload = function (e) {
            document.getElementsByTagName("img")[0].setAttribute("src", e.target.result);
        }
        leer.readAsDataURL(input.files[0]);
    }
}


// TODAS LAS CATEGORIAS
var boton = document.getElementById("btnCategorias");

boton.addEventListener("click", function () {
    window.location.href = "https://localhost:44344/Categoria/Show";
});


//MOSTRAR MENU
//function desplegar() {
//    var elemento = document.getElementById("menuDesplegable");
//    if (elemento.style.display === "block") {
//        elemento.style.display = "none";
//    } else {
//        elemento.style.display = "block";
//    }
//}


//MOSTRAR CATEGORIAS
function desplegar() {
    window.location.href = "https://localhost:44344/Categoria/Show";
}


// REDIRECCIONAR
//var menu = document.getElementById("menuDesple");
///*var urlBase = "https://localhost:44344/Categoria/";*/

//menu.addEventListener("change", function () {
//    var selectedOption = menu.options[menu.selectedIndex].value;

//    if (selectedOption === "1") {
//        window.location.href = "https://localhost:44344/Categoria/Camisas";
//    } else if (selectedOption === "2") {
//        window.location.href = "https://localhost:44344/Categoria/Pantalones";

//    } else if (selectedOption === "3") {
//        window.location.href = "https://i.pinimg.com/736x/68/92/f2/6892f2e4b730eb58502d8fbd3ccc4609.jpg";
//    }
//    else {
//        window.location.href = "https://localhost:44344/Categoria/Show";
//    }
//    //window.location.href = urlBase + opcionSeleccionada;
//})


 //MANTENER OPCION FIJA
//var options = document.queryselectorall('menudesplegable option');

//options.foreach(function (option) {
//    option.addeventlistener('click', function () {
//        options.foreach(function (otheroption) {
//            otheroption.classlist.remove('selected');
//        });
//        this.classlist.add('selected');
//    });
//});