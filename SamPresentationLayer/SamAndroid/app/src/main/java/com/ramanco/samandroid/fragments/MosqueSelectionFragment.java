package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.adapters.MosquesAdapter;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.endpoints.MosquesApiEndpoint;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.UxUtil;
import com.rey.material.widget.EditText;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import retrofit2.Response;

public class MosqueSelectionFragment extends Fragment {

    //region Fields:
    ProgressDialog progress;
    Runnable onMosqueSelectionAction;
    MosqueDto selectedMosque;
    MosqueDto[] allMosques;
    //endregion

    //region Ctors:
    public MosqueSelectionFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_mosque_selection, container, false);

        try {

            loadMosquesAsync();

            //region set on item click listener:
            ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
            lvItems.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    try {
                        MosqueDto mosque = (MosqueDto) parent.getItemAtPosition(position);
                        setSelectedMosque(mosque);
                        if (onMosqueSelectionAction != null)
                            onMosqueSelectionAction.run();
                    } catch (Exception ex) {
                        ExceptionManager.Handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region set on text change listener:
            EditText etSearch = (EditText) fragmentView.findViewById(R.id.et_search);
            etSearch.addTextChangedListener(new TextWatcher() {
                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {
                    try {
                        filterMosques(s.toString());
                    } catch (Exception ex) {
                        ExceptionManager.Handle(getActivity(), ex);
                    }
                }

                @Override
                public void afterTextChanged(Editable s) {

                }
            });
            //endregion

        } catch (Exception ex) {
            if (progress != null)
                progress.dismiss();
            ExceptionManager.Handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Getters & Setters:

    public Runnable getOnMosqueSelectionAction() {
        return onMosqueSelectionAction;
    }

    public void setOnMosqueSelectionAction(Runnable onMosqueSelectionAction) {
        this.onMosqueSelectionAction = onMosqueSelectionAction;
    }

    public MosqueDto getSelectedMosque() {
        return selectedMosque;
    }

    public void setSelectedMosque(MosqueDto selectedMosque) {
        this.selectedMosque = selectedMosque;
    }

    //endregion

    //region Methods:
    private void loadMosquesAsync() throws IOException, CallServerException {

        //region call server and fill list view:
        final int cityId = PrefUtil.getCityID(getActivity());
        progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    MosquesApiEndpoint endpoint = ApiUtil.createEndpoint(MosquesApiEndpoint.class);
                    Response<MosqueDto[]> response = endpoint.findByCity(cityId).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    allMosques = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                MosquesAdapter adapter = new MosquesAdapter(getActivity(), allMosques);
                                View fragmentView = getView();
                                if (fragmentView != null) {
                                    ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
                                    lvItems.setAdapter(adapter);
                                    progress.dismiss();
                                }
                            } catch (Exception ex) {
                                progress.dismiss();
                                ExceptionManager.Handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.Handle(getActivity(), ex);
                }
            }
        }).start();
        //endregion

    }

    private void filterMosques(String name) {
        View fragmentView = getView();
        if (fragmentView != null) {
            // filter mosques:
            List<MosqueDto> filteredMosques = new ArrayList<>();
            for (MosqueDto mosque : allMosques) {
                if (mosque.getName().contains(name) || name.contains(mosque.getName())) {
                    filteredMosques.add(mosque);
                }
            }
            // load list view:
            MosquesAdapter adapter = new MosquesAdapter(getActivity(), filteredMosques.toArray(new MosqueDto[filteredMosques.size()]));
            ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
            lvItems.setAdapter(adapter);

        }
    }
    //endregion

}
