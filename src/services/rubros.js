// Importe de rutas para apis, funciones y utilidades
import {
    RUBROS_API, RUBROS_GET_BY_CODIGO_API, RUBROS_INACTIVAR_PROYECTO_API, RUBROS_COMPLETOS_API, RUBROS_PROYECTO_API
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Obtener todos los rubros y mostrar
const obtenerTodosLosRubros = async () => {
    try {
        const response = await getData(RUBROS_COMPLETOS_API);

        if (response && response.data) {

            // Inicializar DataTable con los datos obtenidos
            $('#tablaRubros').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { data: "codigo" },
                    { data: "nombre" },
                    { data: "codigoProyecto" },
                    { data: "nombreProyecto" },
                    { data: "municipio" },
                    { data: "departamento" },
                    {
                        data: "fechaInicio",
                        render: function (data) {
                            // Formatear fecha para mejor visualización
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    {
                        data: "fechaFin",
                        render: function (data) {
                            // Formatear fecha para mejor visualización
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarProyecto(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarProyecto(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
                                </div>
                            `;
                        }
                    }
                ],
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-copy"></i> Copiar'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-excel"></i> Excel'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-pdf"></i> PDF'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-print"></i> Imprimir'
                    }
                ],
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                },
                responsive: true,
                order: [[0, 'desc']] // Ordenar por ID descendente por defecto
            });
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
    await obtenerTodosLosRubros();
};