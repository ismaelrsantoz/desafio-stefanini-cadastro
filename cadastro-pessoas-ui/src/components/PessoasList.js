import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Typography,
  Box,
  CircularProgress
} from '@mui/material';
import PessoaRow from './PessoaRow'; // 1. Importar o novo componente de linha

function PessoasList({ pessoas, onDelete, onEdit, isLoading }) {

  return (
    <TableContainer component={Paper}>
      <Typography variant="h6" component="div" sx={{ padding: '16px' }}>
        Resultados da Consulta
      </Typography>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', p: 3 }}>
          <CircularProgress />
        </Box>
      ) : pessoas.length === 0 ? (
        <Typography sx={{ padding: '16px' }}>Nenhum resultado encontrado.</Typography>
      ) : (
        // A tabela é definida como "collapsible" para acessibilidade
        <Table aria-label="collapsible table">
          <TableHead>
            <TableRow>
              <TableCell /> {/* Coluna vazia para o botão de expandir */}
              <TableCell sx={{ fontWeight: 'bold' }}>Nome</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>CPF</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Data de Nascimento</TableCell>
              <TableCell align="right" sx={{ fontWeight: 'bold' }}>Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {/* 2. Mapeia as pessoas para o novo componente PessoaRow */}
            {pessoas.map((pessoa) => (
              <PessoaRow key={pessoa.id} pessoa={pessoa} onDelete={onDelete} onEdit={onEdit} />
            ))}
          </TableBody>
        </Table>
      )}
    </TableContainer>
  );
}

export default PessoasList;