// Importe de rutas para apis, funciones y utilidades
import {
    PROYECTOS_API,
    RUBROS_COMPLETOS_API,
    DONACIONES_LISTAR_API,
    ORDENES_COMPRA_COMPLETAS_API,
    DONACIONES_TOP_DONANTES_API
} from '../config/settings.js';
import { showAlert, showError } from '../utils/sweetAlert.js';
import { getData } from '../data/methods.js';

// Función para obtener y mostrar el conteo de proyectos activos
const totalProyectosActivos = async () => {
    try {
        const response = await getData(PROYECTOS_API);
        if (response && response.data) {
            document.getElementById('proyectosActivos').innerText = response.data.length;
        }
    }
    catch (error) {
        showError(error.message, "danger");
        throw error;
    }
}


// Función para obtener y mostrar el conteo de rubros activos
const totalRubrosActivos = async () => {
    try {
        const response = await getData(RUBROS_COMPLETOS_API);
        if (response && response.data) {
            document.getElementById('rubrosActivos').innerText = response.data.length;
        }
    }
    catch (error) {
        showError(error.message, "danger");
        throw error;
    }
}


// Función para obtener y mostrar el conteo de rubros activos
const totalDonacionesActivas = async () => {
    try {
        const response = await getData(DONACIONES_LISTAR_API);
        if (response && response.data) {
            document.getElementById('donacionesActivas').innerText = response.data.length;
        }
    }
    catch (error) {
        showError(error.message, "danger");
        throw error;
    }
}


// Función para obtener y mostrar el conteo de ordenes de compra activas
const totalOrdenesDeCompraActivas = async () => {
    try {
        const response = await getData(ORDENES_COMPRA_COMPLETAS_API);
        if (response && response.data) {
            document.getElementById('ordenesActivas').innerText = response.data.length;
        }
    }
    catch (error) {
        showError(error.message, "danger");
        throw error;
    }
}


// Función para obtener y mostrar el conteo de órdenes de compra activas
const topDonador = async () => {
    try {
        const response = await getData(DONACIONES_TOP_DONANTES_API + "?top=1");
        
        if (response && response.data && response.data.length > 0) {
            document.getElementById('topDonador').innerText = response.data[0].nombreDonante;
        } else {
            document.getElementById('topDonador').innerText = '0';
        }
    }
    catch (error) {
        showError(error.message, "danger");
        throw error;
    }
}


// Inicializar la página
window.onload = async () => {
    await totalProyectosActivos();
    await totalRubrosActivos();
    await totalDonacionesActivas();
    await totalOrdenesDeCompraActivas();
    await topDonador();
};