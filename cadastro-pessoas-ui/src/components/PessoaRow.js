import React, { useState } from 'react';
import {
  TableRow,
  TableCell,
  IconButton,
  Collapse,
  Box,
  Typography,
  Table,
  TableBody,
  Tooltip // 1. IMPORTAR O TOOLTIP
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';

function PessoaRow({ pessoa, onDelete, onEdit }) {
  const [open, setOpen] = useState(false);

  const formatarData = (dataISO) => {
    if (!dataISO) return '';
    return new Date(dataISO).toLocaleDateString('pt-BR');
  };

  const formatarDataHora = (dataISO) => {
    if (!dataISO) return '';
    return new Date(dataISO).toLocaleString('pt-BR');
  };

  return (
    <React.Fragment>
      {/* Linha Principal (Visível) */}
      <TableRow sx={{ '& > *': { borderBottom: 'unset' } }}>
        <TableCell>
          <Tooltip title="Clique para ver mais detalhes">
            <IconButton
              aria-label="expand row"
              size="small"
              onClick={() => setOpen(!open)}
            >
              {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
            </IconButton>
          </Tooltip>
        </TableCell>
        <TableCell component="th" scope="row">
          {pessoa.nome}
        </TableCell>
        <TableCell>{pessoa.cpf}</TableCell>
        <TableCell>{formatarData(pessoa.dataNascimento)}</TableCell>
        <TableCell align="right">
          {/* --- INÍCIO DA ATUALIZAÇÃO --- */}
          <Tooltip title="Editar">
            <IconButton color="primary" onClick={() => onEdit(pessoa)}>
              <EditIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Excluir">
            <IconButton color="error" onClick={() => onDelete(pessoa.id)}>
              <DeleteIcon />
            </IconButton>
          </Tooltip>
          {/* --- FIM DA ATUALIZAÇÃO --- */}
        </TableCell>
      </TableRow>

      {/* Linha de Detalhes (Expansível) */}
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 1 }}>
              <Typography variant="h6" gutterBottom component="div">
                Detalhes
              </Typography>
              <Table size="small" aria-label="detalhes">
                <TableBody>
                  <TableRow>
                    <TableCell component="th" scope="row">E-mail</TableCell>
                    <TableCell>{pessoa.email || 'Não informado'}</TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell component="th" scope="row">Sexo</TableCell>
                    <TableCell>{pessoa.sexo || 'Não informado'}</TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell component="th" scope="row">Naturalidade</TableCell>
                    <TableCell>{pessoa.naturalidade || 'Não informado'}</TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell component="th" scope="row">Nacionalidade</TableCell>
                    <TableCell>{pessoa.nacionalidade || 'Não informado'}</TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell component="th" scope="row">Data de Cadastro</TableCell>
                    <TableCell>{formatarDataHora(pessoa.dataCadastro)}</TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell component="th" scope="row">Última Atualização</TableCell>
                    <TableCell>{formatarDataHora(pessoa.dataAtualizacao)}</TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}

export default PessoaRow;