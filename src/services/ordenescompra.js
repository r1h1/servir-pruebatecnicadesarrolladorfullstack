// Importe de rutas para apis, funciones y utilidades
import {
    ORDENES_COMPRA_API,
    ORDENES_COMPRA_GET_BY_ID_API,
    ORDENES_COMPRA_RUBRO_API,
    ORDENES_COMPRA_COMPLETAS_API,
    ORDENES_COMPRA_TOTAL_RUBRO_API,
    ORDENES_COMPRA_RANGO_FECHAS_API,
    ORDENES_COMPRA_REPORTE_PROYECTO_API,
    ORDENES_COMPRA_BALANCE_RUBRO_API,
    ORDENES_COMPRA_INACTIVAR_RUBRO_API
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';


// Obtener todas las órdenes de compra y mostrar
const obtenerTodasLasOrdenesDeCompra = async () => {
    try {
        const response = await getData(ORDENES_COMPRA_COMPLETAS_API);

        if (response && response.data) {
            console.log(response.data);
        } else {
            showAlert("No se encontraron proyectos.", "warning");
            // Inicializar DataTable vacío para que se muestren los botones de exportación
            $('#tablaProyectos').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary'
                    }
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                }
            });
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

window.onload = async () => {
    await obtenerTodasLasOrdenesDeCompra();
};