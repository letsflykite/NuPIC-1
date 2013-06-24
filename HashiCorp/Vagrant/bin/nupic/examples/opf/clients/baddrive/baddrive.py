#!/usr/bin/env python
# ----------------------------------------------------------------------
# Numenta Platform for Intelligent Computing (NuPIC)
# Copyright (C) 2013, Numenta, Inc.  Unless you have purchased from
# Numenta, Inc. a separate commercial license for this software code, the
# following terms and conditions apply:
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License version 3 as
# published by the Free Software Foundation.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see http://www.gnu.org/licenses.
#
# http://numenta.org/licenses/
# ----------------------------------------------------------------------

"""A simple client to create a CLA model for hotgym."""

import csv
import datetime

from nupic.data.datasethelpers import findDataset
from nupic.frameworks.opf.modelfactory import ModelFactory
from nupic.data import inference_shifter

import model_params

DATA_PATH = "FakeTrafficData.csv"



def createModel():
  return ModelFactory.create(model_params.MODEL_PARAMS)



def runBaddrive():
  model = createModel()
  model.enableInference({'predictionSteps': [1, 5],
                         'predictedField': 'IncidentLat',
                         'numRecords': 1000})
  with open (DATA_PATH) as fin:
    reader = csv.reader(fin)
    headers = reader.next()
    shifter = inference_shifter.InferenceShifter()
    print headers
    print reader.next()
    print reader.next()
    for record in reader:
      #print record
      modelInput = dict(zip(headers, record))
      modelInput["IncidentLat"] = float(modelInput["IncidentLat"])
      modelInput["IncidentType"] = float(modelInput["IncidentType"])
      modelInput["Severity"] = float(modelInput["Severity"])
      modelInput["Verified"] = float(modelInput["Verified"])
      modelInput["IncidentStart"] = datetime.datetime.strptime(
          modelInput["IncidentStart"], "%m/%d/%Y %H:%M:%S %p")
      result = shifter.shift(model.run(modelInput))
      #print result.rawInput['IncidentStart'], '##', result.rawInput['IncidentLat'], '##', result.inferences['multiStepBestPredictions'][1], '##', result.inferences['multiStepBestPredictions'][5]
      print result.rawInput['IncidentStart'], '##', result.rawInput['IncidentLat'], '##', result.inferences['multiStepPredictions'][1], '##', result.inferences['multiStepPredictions'][5]


if __name__ == "__main__":
  runBaddrive()
