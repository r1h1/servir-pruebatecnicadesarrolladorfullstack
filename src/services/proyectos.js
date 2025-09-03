// Importe de rutas para apis, funciones y utilidades
import { PROYECTOS_API, PROYECTOS_GET_BY_CODIGO_API, PROYECTOS_ULTIMO_CODIGO_API } from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Obtener todos los proyectos
const obtenerTodosLosProyectos = async () => {
    try {
        const response = await getData(PROYECTOS_API);

        if (response && response.data) {
            console.log('Proyectos obtenidos:', response.data);
            return response.data;
        } else {
            showAlert("No se encontraron proyectos.", "warning");
            return [];
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

window.onload = async () => {
    await obtenerTodosLosProyectos();
};