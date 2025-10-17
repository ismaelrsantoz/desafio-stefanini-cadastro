import React, { useState, useEffect } from 'react';
import './PessoaForm.css';
import { 
  Button, 
  TextField, 
  Box,
  Select,
  MenuItem,
  FormControl,
  InputLabel
} from '@mui/material';
import { toast } from 'react-toastify';

const initialState = {
  nome: '',
  cpf: '',
  email: '',
  dataNascimento: '',
  sexo: '',
  naturalidade: '',
  nacionalidade: ''
};

function PessoaForm({ onSave, pessoaParaEditar, onCancelEdit }) {
  const [formData, setFormData] = useState(initialState);
  const isEditing = !!pessoaParaEditar;

  useEffect(() => {
    if (pessoaParaEditar) {
      const dataFormatada = pessoaParaEditar.dataNascimento ? pessoaParaEditar.dataNascimento.split('T')[0] : '';
      setFormData({ ...pessoaParaEditar, dataNascimento: dataFormatada });
    } else {
      setFormData(initialState);
    }
  }, [pessoaParaEditar]);

  const handleChange = (e) => {
    const { name, value } = e.target;

    // --- LÓGICA PARA LIMITAR O CAMPO CPF ---
    if (name === 'cpf') {
      // 1. Remove qualquer caractere que não seja número
      const onlyNums = value.replace(/[^0-9]/g, '');
      // 2. Limita a 11 dígitos
      const truncatedValue = onlyNums.slice(0, 11);
      // 3. Atualiza o estado
      setFormData({ ...formData, [name]: truncatedValue });
    } else {
      // Para todos os outros campos, o comportamento é o mesmo de antes
      setFormData({ ...formData, [name]: value });
    }
    // --- FIM DA LÓGICA DO CPF ---
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Validação de data no front-end
    const birthDate = new Date(formData.dataNascimento);
    const today = new Date();
    today.setHours(0, 0, 0, 0); 
    
    if (birthDate > today) {
      toast.error("A data de nascimento não pode ser uma data futura.");
      return;
    }

    const minYear = new Date().getFullYear() - 120;
    if (birthDate.getFullYear() < minYear) {
      toast.error("Idade inválida (superior a 120 anos).");
      return;
    }
    
    const dataToSend = { ...formData };

    if (dataToSend.email === '') {
      dataToSend.email = null;
    }

    onSave(dataToSend);
  };

  return (
    <Box 
      component="form" 
      className="pessoa-form" 
      onSubmit={handleSubmit}
      sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
    >
      <h2>{isEditing ? 'Editar Pessoa' : 'Formulário de Cadastro'}</h2>
      
      <Box sx={{ display: 'flex', gap: 2 }}>
        <TextField label="Nome" name="nome" value={formData.nome} onChange={handleChange} variant="outlined" required fullWidth />
        <TextField 
            label="CPF" 
            name="cpf" 
            value={formData.cpf} 
            onChange={handleChange} 
            variant="outlined" 
            required 
            fullWidth 
            inputProps={{ maxLength: 11 }} // Propriedade extra para UX
        />
      </Box>
      <Box sx={{ display: 'flex', gap: 2 }}>
        <TextField label="E-mail" name="email" value={formData.email || ''} onChange={handleChange} variant="outlined" fullWidth />
        
        <TextField 
          label="Data de Nascimento" 
          name="dataNascimento" 
          type="date" 
          value={formData.dataNascimento} 
          onChange={handleChange} 
          variant="outlined" 
          required 
          fullWidth 
          InputLabelProps={{ shrink: true }}
        />
      </Box>
      <Box sx={{ display: 'flex', gap: 2 }}>
        
        <FormControl fullWidth>
          <InputLabel id="sexo-select-label">Sexo</InputLabel>
          <Select
            labelId="sexo-select-label"
            id="sexo-select"
            name="sexo"
            value={formData.sexo}
            label="Sexo"
            onChange={handleChange}
          >
            <MenuItem value={"Masculino"}>Masculino</MenuItem>
            <MenuItem value={"Feminino"}>Feminino</MenuItem>
          </Select>
        </FormControl>

        <TextField label="Naturalidade" name="naturalidade" value={formData.naturalidade} onChange={handleChange} variant="outlined" fullWidth />
        <TextField label="Nacionalidade" name="nacionalidade" value={formData.nacionalidade} onChange={handleChange} variant="outlined" fullWidth />
      </Box>
      
      <Button variant="contained" color="primary" type="submit" size="large">
          {isEditing ? 'Salvar Alterações' : 'Cadastrar'}
      </Button>

      {isEditing && (
        <Button variant="outlined" color="secondary" onClick={onCancelEdit} size="large">
          Cancelar Edição
        </Button>
      )}
    </Box>
  );
}

export default PessoaForm;

